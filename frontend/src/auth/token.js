const STORAGE_KEY = "nextuniverse.accessToken";

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
