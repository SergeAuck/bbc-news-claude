import { useQuery } from "@tanstack/react-query";
import { motion, AnimatePresence } from "framer-motion";
import { fetchNews } from "./api";
import { NewsCard } from "./components/NewsCard";
import { NewsTicker } from "./components/NewsTicker";
import { LoadingSpinner } from "./components/LoadingSpinner";
import "./App.css";

function App() {
  const {
    data: news,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["news"],
    queryFn: fetchNews,
    refetchInterval: 3 * 60 * 1000,
    staleTime: 3 * 60 * 1000,
  });

  return (
    <div className="app">
      <motion.header
        className="app-header"
        initial={{ y: -100 }}
        animate={{ y: 0 }}
        transition={{ type: "spring", stiffness: 100 }}
      >
        <div className="header-content">
          <h1 className="app-title">
            <span className="title-accent">AUS</span> News
          </h1>
          <p className="app-subtitle">Top Headlines from BBC</p>
        </div>
      </motion.header>

      {news && news.articles.length > 0 && (
        <NewsTicker articles={news.articles} />
      )}

      <main className="main-content">
        {isLoading && <LoadingSpinner />}

        {error && (
          <motion.div
            className="error-message"
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
          >
            <h2>Oops! Something went wrong</h2>
            <p>{(error as Error).message}</p>
          </motion.div>
        )}

        <AnimatePresence>
          {news && (
            <motion.div
              className="news-grid"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5 }}
            >
              {news.articles.map((article, index) => (
                <NewsCard
                  key={article.url || index}
                  article={article}
                  index={index}
                />
              ))}
            </motion.div>
          )}
        </AnimatePresence>
      </main>

      <footer className="app-footer">
        <p>
          Powered by{" "}
          <a
            href="https://newsapi.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            NewsAPI
          </a>
        </p>
      </footer>
    </div>
  );
}

export default App;
