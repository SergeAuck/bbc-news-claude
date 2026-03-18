import type { NewsApiResponse } from "./types";

const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:5262";

export async function fetchNews(): Promise<NewsApiResponse> {
  const response = await fetch(`${API_BASE}/api/news`);
  if (!response.ok) {
    throw new Error(`Failed to fetch news: ${response.statusText}`);
  }
  return response.json();
}
