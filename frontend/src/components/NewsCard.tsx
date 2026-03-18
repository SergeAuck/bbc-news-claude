import { motion } from "framer-motion";
import type { Article } from "../types";

interface NewsCardProps {
  article: Article;
  index: number;
}

export function NewsCard({ article, index }: NewsCardProps) {
  const fallbackImage =
    "https://images.unsplash.com/photo-1504711434969-e33886168d6c?w=600&q=80";

  return (
    <motion.article
      className="news-card"
      initial={{ opacity: 0, y: 60 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: index * 0.1 }}
      whileHover={{ scale: 1.02, boxShadow: "0 20px 40px rgba(0,0,0,0.2)" }}
    >
      <div className="news-card-image">
        <img
          src={article.urlToImage || fallbackImage}
          alt={article.title}
          onError={(e) => {
            (e.target as HTMLImageElement).src = fallbackImage;
          }}
        />
        <div className="news-card-overlay" />
      </div>
      <div className="news-card-content">
        <span className="news-card-source">
          {article.source?.name || "BBC News"}
        </span>
        <h2 className="news-card-title">{article.title}</h2>
        {article.description && (
          <p className="news-card-description">{article.description}</p>
        )}
        <div className="news-card-footer">
          {article.publishedAt && (
            <time className="news-card-date">
              {new Date(article.publishedAt).toLocaleDateString("en-AU", {
                day: "numeric",
                month: "short",
                year: "numeric",
                hour: "2-digit",
                minute: "2-digit",
              })}
            </time>
          )}
          {article.url && (
            <a
              href={article.url}
              target="_blank"
              rel="noopener noreferrer"
              className="news-card-link"
            >
              Read more &rarr;
            </a>
          )}
        </div>
      </div>
    </motion.article>
  );
}
