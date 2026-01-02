const STORAGE_KEY = "nextuniverse.accessToken";

const ROLE_CLAIM_URI =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

function base64UrlDecode(input) {
  const base64 = input.replace(/-/g, "+").replace(/_/g, "/");
  const pad = base64.length % 4;
  const padded = pad ? base64 + "=".repeat(4 - pad) : base64;
  // atob is available in browsers
  return atob(padded);
}

export function decodeJwtPayload(token) {
  if (!token) return null;
  const parts = token.split(".");
  if (parts.length !== 3) return null;
  try {
    const json = base64UrlDecode(parts[1]);
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getUserRoleFromToken(token) {
  const payload = decodeJwtPayload(token);
  if (!payload) return null;

  const roleValue = payload.role ?? payload[ROLE_CLAIM_URI] ?? payload.roles;
  if (!roleValue) return null;

  // role can be a string or an array
  if (Array.isArray(roleValue)) {
    return roleValue[0] ?? null;
  }

  return typeof roleValue === "string" ? roleValue : null;
}

export function isInstructorToken(token) {
  const role = getUserRoleFromToken(token);
  return role === "Instructor";
}

export function getAccessToken() {
  return localStorage.getItem(STORAGE_KEY);
}

export function setAccessToken(token) {
  if (!token) return;
  localStorage.setItem(STORAGE_KEY, token);
}

export function clearAccessToken() {
  localStorage.removeItem(STORAGE_KEY);
}
