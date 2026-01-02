import { getAccessToken } from "../auth/token";

const API_BASE_URL =
  process.env.REACT_APP_API_BASE_URL || "http://localhost:5095";

async function readJsonSafely(response) {
  const contentType = response.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    return response.json();
  }
  const text = await response.text();
  return text;
}

export async function apiFetch(
  path,
  { method = "GET", body, auth = false } = {}
) {
  const headers = {
    Accept: "application/json",
  };

  if (body !== undefined) {
    headers["Content-Type"] = "application/json";
  }

  if (auth) {
    const token = getAccessToken();
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }
  }

  const res = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!res.ok) {
    const payload = await readJsonSafely(res);
    const message =
      typeof payload === "string"
        ? payload
        : payload?.title || payload?.message || JSON.stringify(payload);
    const err = new Error(message || `Request failed: ${res.status}`);
    err.status = res.status;
    err.payload = payload;
    throw err;
  }

  if (res.status === 204) return null;
  return readJsonSafely(res);
}
