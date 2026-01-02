import { apiFetch } from "./client";

export function register({ name, email, password }) {
  return apiFetch("/auth/register", {
    method: "POST",
    body: { name, email, password },
  });
}

export function login({ email, password }) {
  return apiFetch("/auth/login", {
    method: "POST",
    body: { email, password },
  });
}
