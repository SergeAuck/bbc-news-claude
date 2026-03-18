import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { NewsCard } from "./NewsCard";
import type { Article } from "../types";

const mockArticle: Article = {
  source: { id: "bbc-news", name: "BBC News" },
  author: "BBC Reporter",
  title: "Breaking News Title",
  description: "This is a test description",
  url: "https://bbc.com/article",
  urlToImage: "https://example.com/image.jpg",
  publishedAt: "2024-06-15T10:30:00Z",
  content: "Full article content",
};

describe("NewsCard", () => {
  it("renders article title", () => {
    render(<NewsCard article={mockArticle} index={0} />);
    expect(screen.getByText("Breaking News Title")).toBeInTheDocument();
  });

  it("renders article description", () => {
    render(<NewsCard article={mockArticle} index={0} />);
    expect(screen.getByText("This is a test description")).toBeInTheDocument();
  });

  it("renders source name", () => {
    render(<NewsCard article={mockArticle} index={0} />);
    expect(screen.getByText("BBC News")).toBeInTheDocument();
  });

  it("renders read more link", () => {
    render(<NewsCard article={mockArticle} index={0} />);
    const link = screen.getByRole("link", { name: /read more/i });
    expect(link).toHaveAttribute("href", "https://bbc.com/article");
    expect(link).toHaveAttribute("target", "_blank");
  });

  it("handles missing description", () => {
    const articleNoDesc = { ...mockArticle, description: null };
    render(<NewsCard article={articleNoDesc} index={0} />);
    expect(screen.getByText("Breaking News Title")).toBeInTheDocument();
    expect(screen.queryByText("This is a test description")).not.toBeInTheDocument();
  });
});
