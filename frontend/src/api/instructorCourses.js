import { apiFetch } from "./client";

export function listMyCourses() {
  return apiFetch("/instructor/courses", { auth: true });
}

export function createCourse({ title, description, language, level }) {
  return apiFetch("/instructor/courses", {
    method: "POST",
    auth: true,
    body: { title, description, language, level },
  });
}

export function publishCourse(courseId) {
  return apiFetch(`/instructor/courses/${courseId}/publish`, {
    method: "POST",
    auth: true,
  });
}

export function unpublishCourse(courseId) {
  return apiFetch(`/instructor/courses/${courseId}/unpublish`, {
    method: "POST",
    auth: true,
  });
}
