import { apiFetch } from "./client";

export function listCourses() {
  return apiFetch("/courses");
}

export function getCourse(id) {
  return apiFetch(`/courses/${id}`);
}
