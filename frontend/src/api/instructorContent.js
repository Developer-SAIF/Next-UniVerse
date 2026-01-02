import { apiFetch } from "./client";

export function listModules(courseId) {
  return apiFetch(`/instructor/courses/${courseId}/modules`, { auth: true });
}

export function createModule(courseId, { title, position, description }) {
  return apiFetch(`/instructor/courses/${courseId}/modules`, {
    method: "POST",
    auth: true,
    body: { title, position, description },
  });
}

export function updateModule(moduleId, { title, position, description }) {
  return apiFetch(`/instructor/modules/${moduleId}`, {
    method: "PUT",
    auth: true,
    body: { title, position, description },
  });
}

export function deleteModule(moduleId) {
  return apiFetch(`/instructor/modules/${moduleId}`, {
    method: "DELETE",
    auth: true,
  });
}

export function createMaterial(moduleId, payload) {
  return apiFetch(`/instructor/modules/${moduleId}/materials`, {
    method: "POST",
    auth: true,
    body: payload,
  });
}

export function updateMaterial(materialId, payload) {
  return apiFetch(`/instructor/materials/${materialId}`, {
    method: "PUT",
    auth: true,
    body: payload,
  });
}

export function deleteMaterial(materialId) {
  return apiFetch(`/instructor/materials/${materialId}`, {
    method: "DELETE",
    auth: true,
  });
}
