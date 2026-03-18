import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { describe, it, expect, vi, beforeEach } from "vitest";
import App from "./App";

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

vi.mock("./api", () => ({
  fetchNews: vi.fn(),
}));

import { fetchNews } from "./api";
const mockFetchNews = vi.mocked(fetchNews);

describe("App", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders the header", () => {
    mockFetchNews.mockResolvedValue({
      status: "ok",
      totalResults: 0,
      articles: [],
    });

    render(<App />, { wrapper: createWrapper() });
    expect(screen.getByText("AUS")).toBeInTheDocument();
    expect(screen.getByText("News")).toBeInTheDocument();
  });

  it("shows loading state", () => {
    mockFetchNews.mockReturnValue(new Promise(() => {}));

    render(<App />, { wrapper: createWrapper() });
    expect(screen.getByText("Fetching latest news...")).toBeInTheDocument();
  });

  it("renders articles when loaded", async () => {
    mockFetchNews.mockResolvedValue({
      status: "ok",
      totalResults: 1,
      articles: [
        {
          source: { id: "bbc-news", name: "BBC News" },
          author: "BBC",
          title: "Test BBC Article",
          description: "A test article description",
          url: "https://bbc.com/test",
          urlToImage: null,
          publishedAt: "2024-01-01T00:00:00Z",
          content: "Full content here",
        },
      ],
    });

    render(<App />, { wrapper: createWrapper() });

    expect(await screen.findByText("Test BBC Article")).toBeInTheDocument();
    expect(
      screen.getByText("A test article description")
    ).toBeInTheDocument();
  });

  it("shows error state", async () => {
    mockFetchNews.mockRejectedValue(new Error("Network error"));

    render(<App />, { wrapper: createWrapper() });

    expect(
      await screen.findByText("Oops! Something went wrong")
    ).toBeInTheDocument();
    expect(screen.getByText("Network error")).toBeInTheDocument();
  });
});
