-- Full schema for Online Learning Platform
-- PostgreSQL (tested on 13+). Adjust types/lengths as needed.

-- === Extensions ===
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- === ENUM types ===
CREATE TYPE user_role AS ENUM ('student','instructor','admin');
CREATE TYPE user_status AS ENUM ('active','suspended','pending');
CREATE TYPE course_level AS ENUM ('beginner','intermediate','advanced');
CREATE TYPE enrollment_status AS ENUM ('active','completed','cancelled');
CREATE TYPE material_kind AS ENUM ('video','pdf','slide','html','audio','other');
CREATE TYPE question_type AS ENUM ('mcq','multi_select','short_text','long_text','numeric');
CREATE TYPE quiz_attempt_status AS ENUM ('in_progress','submitted','graded','abandoned');
CREATE TYPE submission_status AS ENUM ('submitted','graded','late','resubmitted');
CREATE TYPE certificate_status AS ENUM ('issued','revoked');
CREATE TYPE notification_type AS ENUM ('system','course','assignment','message');
CREATE TYPE payment_status AS ENUM ('pending','completed','failed','refunded');
CREATE TYPE relationship_type AS ENUM ('follower','following','blocked','colleague');

-- === Users ===
CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    role user_role NOT NULL DEFAULT 'student',
    name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    password_hash VARCHAR(255),
    status user_status NOT NULL DEFAULT 'pending',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE
);

-- Optional self-referential FK for created_by/updated_by (nullable)
ALTER TABLE users
  ADD CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE users
  ADD CONSTRAINT fk_users_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Courses ===
CREATE TABLE courses (
    id BIGSERIAL PRIMARY KEY,
    instructor_id BIGINT NOT NULL,
    title VARCHAR(255),
    description TEXT,
    language VARCHAR(50),
    level course_level DEFAULT 'beginner',
    is_published BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE courses
  ADD CONSTRAINT fk_courses_instructor FOREIGN KEY (instructor_id) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE courses
  ADD CONSTRAINT fk_courses_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE courses
  ADD CONSTRAINT fk_courses_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Enrollments ===
CREATE TABLE enrollments (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    course_id BIGINT NOT NULL,
    status enrollment_status NOT NULL DEFAULT 'active',
    enrolled_on DATE DEFAULT CURRENT_DATE,
    completed_on DATE,
    progress NUMERIC(5,2) DEFAULT 0 CHECK (progress >= 0 AND progress <= 100),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    UNIQUE (user_id, course_id)
);

ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollments_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE enrollments
  ADD CONSTRAINT fk_enrollments_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;

-- === Modules ===
CREATE TABLE modules (
    id BIGSERIAL PRIMARY KEY,
    course_id BIGINT NOT NULL,
    title VARCHAR(255),
    position INTEGER,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE,
    UNIQUE (course_id, position)
);

ALTER TABLE modules
  ADD CONSTRAINT fk_modules_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;
ALTER TABLE modules
  ADD CONSTRAINT fk_modules_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE modules
  ADD CONSTRAINT fk_modules_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Materials ===
CREATE TABLE materials (
    id BIGSERIAL PRIMARY KEY,
    module_id BIGINT NOT NULL,
    kind material_kind NOT NULL,
    title VARCHAR(255),
    description TEXT,
    storage_url VARCHAR(1024),
    mime_type VARCHAR(255),
    size_bytes BIGINT,
    requires_auth BOOLEAN DEFAULT TRUE,
    position INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE materials
  ADD CONSTRAINT fk_materials_module FOREIGN KEY (module_id) REFERENCES modules(id) ON DELETE CASCADE;
ALTER TABLE materials
  ADD CONSTRAINT fk_materials_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE materials
  ADD CONSTRAINT fk_materials_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Video Streams ===
CREATE TABLE video_streams (
    id BIGSERIAL PRIMARY KEY,
    material_id BIGINT NOT NULL,
    provider VARCHAR(100),
    stream_url VARCHAR(1024),
    playback_policy VARCHAR(50),
    duration_seconds INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE video_streams
  ADD CONSTRAINT fk_video_streams_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE CASCADE;

-- === Quizzes ===
CREATE TABLE quizzes (
    id BIGSERIAL PRIMARY KEY,
    module_id BIGINT NOT NULL,
    title VARCHAR(255),
    time_limit_seconds INTEGER,
    attempts_allowed INTEGER DEFAULT 1,
    shuffle_questions BOOLEAN DEFAULT FALSE,
    pass_score NUMERIC(5,2),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE quizzes
  ADD CONSTRAINT fk_quizzes_module FOREIGN KEY (module_id) REFERENCES modules(id) ON DELETE CASCADE;
ALTER TABLE quizzes
  ADD CONSTRAINT fk_quizzes_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE quizzes
  ADD CONSTRAINT fk_quizzes_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Questions ===
CREATE TABLE questions (
    id BIGSERIAL PRIMARY KEY,
    quiz_id BIGINT NOT NULL,
    type question_type NOT NULL,
    prompt TEXT,
    points NUMERIC(8,2) DEFAULT 0,
    position INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE questions
  ADD CONSTRAINT fk_questions_quiz FOREIGN KEY (quiz_id) REFERENCES quizzes(id) ON DELETE CASCADE;

-- === Choices ===
CREATE TABLE choices (
    id BIGSERIAL PRIMARY KEY,
    question_id BIGINT NOT NULL,
    text TEXT,
    is_correct BOOLEAN DEFAULT FALSE,
    position INTEGER,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE choices
  ADD CONSTRAINT fk_choices_question FOREIGN KEY (question_id) REFERENCES questions(id) ON DELETE CASCADE;

-- === Quiz Attempts ===
CREATE TABLE quiz_attempts (
    id BIGSERIAL PRIMARY KEY,
    quiz_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    submitted_at TIMESTAMP WITH TIME ZONE,
    score NUMERIC(8,2),
    status quiz_attempt_status DEFAULT 'in_progress',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE quiz_attempts
  ADD CONSTRAINT fk_quiz_attempts_quiz FOREIGN KEY (quiz_id) REFERENCES quizzes(id) ON DELETE CASCADE;
ALTER TABLE quiz_attempts
  ADD CONSTRAINT fk_quiz_attempts_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

CREATE INDEX idx_quiz_attempts_quiz_user ON quiz_attempts (quiz_id, user_id);

-- === Attempt Answers ===
CREATE TABLE attempt_answers (
    id BIGSERIAL PRIMARY KEY,
    attempt_id BIGINT NOT NULL,
    question_id BIGINT NOT NULL,
    answer_text TEXT,
    selected_choice_id BIGINT,
    is_correct BOOLEAN,
    awarded_points NUMERIC(8,2),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT chk_answer_presence CHECK (
        (answer_text IS NOT NULL) OR (selected_choice_id IS NOT NULL)
    )
);

ALTER TABLE attempt_answers
  ADD CONSTRAINT fk_attempt_answers_attempt FOREIGN KEY (attempt_id) REFERENCES quiz_attempts(id) ON DELETE CASCADE;
ALTER TABLE attempt_answers
  ADD CONSTRAINT fk_attempt_answers_question FOREIGN KEY (question_id) REFERENCES questions(id) ON DELETE CASCADE;
ALTER TABLE attempt_answers
  ADD CONSTRAINT fk_attempt_answers_choice FOREIGN KEY (selected_choice_id) REFERENCES choices(id) ON DELETE SET NULL;

-- === Assignments ===
CREATE TABLE assignments (
    id BIGSERIAL PRIMARY KEY,
    module_id BIGINT NOT NULL,
    title VARCHAR(255),
    instructions TEXT,
    due_at TIMESTAMP WITH TIME ZONE,
    max_points NUMERIC(8,2),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT,
    updated_by BIGINT,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE assignments
  ADD CONSTRAINT fk_assignments_module FOREIGN KEY (module_id) REFERENCES modules(id) ON DELETE CASCADE;
ALTER TABLE assignments
  ADD CONSTRAINT fk_assignments_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;
ALTER TABLE assignments
  ADD CONSTRAINT fk_assignments_updated_by FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Submissions ===
CREATE TABLE submissions (
    id BIGSERIAL PRIMARY KEY,
    assignment_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    submitted_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    storage_url VARCHAR(1024),
    grade NUMERIC(8,2),
    feedback TEXT,
    status submission_status DEFAULT 'submitted',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE submissions
  ADD CONSTRAINT fk_submissions_assignment FOREIGN KEY (assignment_id) REFERENCES assignments(id) ON DELETE CASCADE;
ALTER TABLE submissions
  ADD CONSTRAINT fk_submissions_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

-- If only one submission per user per assignment is allowed, uncomment:
-- ALTER TABLE submissions ADD CONSTRAINT uq_submissions_assignment_user UNIQUE (assignment_id, user_id);

-- === Certificates ===
CREATE TABLE certificates (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    course_id BIGINT NOT NULL,
    serial VARCHAR(255) UNIQUE,
    issued_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    status certificate_status DEFAULT 'issued',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE certificates
  ADD CONSTRAINT fk_certificates_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE certificates
  ADD CONSTRAINT fk_certificates_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;

ALTER TABLE certificates ADD CONSTRAINT uq_certificates_user_course UNIQUE (user_id, course_id);

-- === Discussion Threads ===
CREATE TABLE discussion_threads (
    id BIGSERIAL PRIMARY KEY,
    course_id BIGINT NOT NULL,
    module_id BIGINT,
    title VARCHAR(255),
    is_locked BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    created_by BIGINT
);

ALTER TABLE discussion_threads
  ADD CONSTRAINT fk_discussion_threads_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;
ALTER TABLE discussion_threads
  ADD CONSTRAINT fk_discussion_threads_module FOREIGN KEY (module_id) REFERENCES modules(id) ON DELETE SET NULL;
ALTER TABLE discussion_threads
  ADD CONSTRAINT fk_discussion_threads_created_by FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE SET NULL;

-- === Posts ===
CREATE TABLE posts (
    id BIGSERIAL PRIMARY KEY,
    thread_id BIGINT NOT NULL,
    author_id BIGINT NOT NULL,
    body TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE posts
  ADD CONSTRAINT fk_posts_thread FOREIGN KEY (thread_id) REFERENCES discussion_threads(id) ON DELETE CASCADE;
ALTER TABLE posts
  ADD CONSTRAINT fk_posts_author FOREIGN KEY (author_id) REFERENCES users(id) ON DELETE SET NULL;

CREATE INDEX idx_posts_thread_created_at ON posts (thread_id, created_at);

-- === Notifications ===
CREATE TABLE notifications (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    type notification_type NOT NULL,
    message TEXT,
    is_read BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE notifications
  ADD CONSTRAINT fk_notifications_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

-- === Payments ===
CREATE TABLE payments (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    course_id BIGINT,
    amount NUMERIC(12,2),
    currency VARCHAR(10),
    status payment_status DEFAULT 'pending',
    payment_date TIMESTAMP WITH TIME ZONE,
    provider VARCHAR(255),
    transaction_id VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE payments
  ADD CONSTRAINT fk_payments_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE payments
  ADD CONSTRAINT fk_payments_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE SET NULL;

-- === Analytics Events ===
CREATE TABLE analytics_events (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT,
    event_type VARCHAR(255),
    event_data JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

ALTER TABLE analytics_events
  ADD CONSTRAINT fk_analytics_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL;

CREATE INDEX idx_analytics_event_type_created_at ON analytics_events (event_type, created_at);

-- === User Relationships ===
CREATE TABLE user_relationships (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    related_user_id BIGINT NOT NULL,
    relationship_type relationship_type NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    UNIQUE (user_id, related_user_id)
);

ALTER TABLE user_relationships
  ADD CONSTRAINT fk_user_relationships_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
ALTER TABLE user_relationships
  ADD CONSTRAINT fk_user_relationships_related_user FOREIGN KEY (related_user_id) REFERENCES users(id) ON DELETE CASCADE;

-- === Course Reviews ===
CREATE TABLE course_reviews (
    id BIGSERIAL PRIMARY KEY,
    course_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL,
    rating INTEGER CHECK (rating >= 1 AND rating <= 5),
    review_text TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updated_at TIMESTAMP WITH TIME ZONE,
    deleted_at TIMESTAMP WITH TIME ZONE
);

ALTER TABLE course_reviews
  ADD CONSTRAINT fk_course_reviews_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;
ALTER TABLE course_reviews
  ADD CONSTRAINT fk_course_reviews_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

ALTER TABLE course_reviews ADD CONSTRAINT uq_course_reviews_user_course UNIQUE (user_id, course_id);

CREATE INDEX idx_course_reviews_course_created_at ON course_reviews (course_id, created_at);

-- === Helpful additional indexes ===
CREATE INDEX idx_users_email ON users (email);
CREATE INDEX idx_courses_instructor ON courses (instructor_id);
CREATE INDEX idx_modules_course_position ON modules (course_id, position);
CREATE INDEX idx_materials_module_position ON materials (module_id, position);

-- === Audit trigger helpers (optional) ===
-- Example: auto-update updated_at timestamp
CREATE OR REPLACE FUNCTION trigger_set_timestamp()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = now();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Attach trigger to tables that use updated_at
DO $$
BEGIN
  -- list tables to attach trigger
  PERFORM 1 FROM pg_proc WHERE proname = 'trigger_set_timestamp';
  -- Attach triggers
  EXECUTE 'CREATE TRIGGER trg_users_set_timestamp BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_courses_set_timestamp BEFORE UPDATE ON courses FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_modules_set_timestamp BEFORE UPDATE ON modules FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_materials_set_timestamp BEFORE UPDATE ON materials FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_quizzes_set_timestamp BEFORE UPDATE ON quizzes FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_questions_set_timestamp BEFORE UPDATE ON questions FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_choices_set_timestamp BEFORE UPDATE ON choices FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_quiz_attempts_set_timestamp BEFORE UPDATE ON quiz_attempts FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_attempt_answers_set_timestamp BEFORE UPDATE ON attempt_answers FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_assignments_set_timestamp BEFORE UPDATE ON assignments FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_submissions_set_timestamp BEFORE UPDATE ON submissions FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
  EXECUTE 'CREATE TRIGGER trg_course_reviews_set_timestamp BEFORE UPDATE ON course_reviews FOR EACH ROW EXECUTE FUNCTION trigger_set_timestamp()';
EXCEPTION WHEN duplicate_object THEN
  -- ignore if triggers already exist
  NULL;
END;
$$;

-- === End of schema ===
