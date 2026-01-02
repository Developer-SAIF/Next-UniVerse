import { apiFetch } from "./client";

export function enroll(courseId) {
  return apiFetch("/enrollments", {
    method: "POST",
    auth: true,
    body: { courseId },
  });
}

export function myEnrollments() {
  return apiFetch("/enrollments/me", { auth: true });
}
