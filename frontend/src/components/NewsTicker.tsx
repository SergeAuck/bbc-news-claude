import { motion } from "framer-motion";
import type { Article } from "../types";

interface NewsTickerProps {
  articles: Article[];
}

export function NewsTicker({ articles }: NewsTickerProps) {
  const tickerText = articles.map((a) => a.title).join("  \u2022  ");
  const doubled = `${tickerText}  \u2022  ${tickerText}`;

  return (
    <div className="news-ticker" aria-label="Breaking news ticker">
      <span className="ticker-label">BREAKING</span>
      <div className="ticker-track">
        <motion.div
          className="ticker-content"
          animate={{ x: ["0%", "-50%"] }}
          transition={{
            x: {
              repeat: Infinity,
              repeatType: "loop",
              duration: articles.length * 5,
              ease: "linear",
            },
          }}
        >
          {doubled}
        </motion.div>
      </div>
    </div>
  );
}
