// src/Pages/UserPage.jsx
import { useMemo, useState } from "react";
import { useAuth } from "../context/AuthContext";
import MarioStyles from "../Components/mario/MarioStyles";
import Cloud from "../Components/mario/Cloud";
import Coin from "../Components/mario/Coin";
import QuestionBlock from "../Components/mario/QuestionBlock";
import TopPanel from "../Components/mario/TopPanel";
import Ground from "../Components/mario/Ground";
import { spawnCoin } from "../utils/spawnCoin";

export default function UserPage() {
  const { user, logout } = useAuth();

  // game state
  const [score, setScore] = useState(0);
  const [coins, setCoins] = useState(() => Array.from({ length: 6 }, spawnCoin));

  // audio (small pling)
  const coinAudio = useMemo(
    () => new Audio("data:audio/wav;base64,UklGRoQAAABXQVZFZm10IBAAAAABAAEAESsAACJWAAACABYAAAEsAAACABYAAABbAAAASAAAAGZmZmZmZmY="),
    []
  );

  const collectCoin = (id) => {
    setCoins((prev) => prev.map((c) => (c.id === id ? { ...c, collected: true } : c)));
    setScore((s) => s + 1);
    try { coinAudio.currentTime = 0; coinAudio.play().catch(() => {}); } catch {}
    setTimeout(() => {
      setCoins((prev) => prev.map((c) => (c.id === id ? spawnCoin() : c)));
    }, 350);
  };

  const username = user?.firstname || user?.email || "Användare";

  return (
    <div
      style={{
        minHeight: "100vh",
        overflow: "hidden",
        background: "linear-gradient(180deg, #6ec6ff 0%, #8bc34a 100%)",
        fontFamily: "'Press Start 2P', cursive",
        color: "#fff",
        textShadow: "2px 2px #000",
        position: "relative",
      }}
    >
      {/* Global styles/animations */}
      <MarioStyles />

      {/* Top UI */}
      <TopPanel score={score} username={username} onLogout={logout} />

      {/* Clouds */}
      <Cloud left="-15vw" duration="28s" />
      <Cloud left="-30vw" top="14vh" width={160} duration="38s" />
      <Cloud left="-45vw" top="22vh" width={100} height={50} duration="24s" />

      {/* Question blocks */}
      <QuestionBlock left="20vw" top="62vh" delay="0s" />
      <QuestionBlock left="50vw" top="62vh" delay="0.3s" />
      <QuestionBlock left="75vw" top="62vh" delay="0.6s" />

      {/* Coins */}
      {coins.map((c) => (
        <Coin key={c.id} x={c.x} y={c.y} collected={c.collected} onClick={() => !c.collected && collectCoin(c.id)} />
      ))}

      {/* Content */}
      <div
        style={{
          position: "relative",
          zIndex: 2,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          padding: "10vh 16px 120px",
          textAlign: "center",
        }}
      >
        <h1
          style={{
            fontSize: 22,
            marginBottom: 14,
            color: "#ffcc00",
            textShadow: "3px 3px #d32f2f, 5px 5px #000",
          }}
        >
          🍄 Välkommen {username}!
        </h1>
        <p
          style={{
            fontSize: 12,
            background: "rgba(0,0,0,0.35)",
            padding: "10px 16px",
            borderRadius: 8,
            marginBottom: 24,
            maxWidth: 560,
          }}
        >
          Samla mynt genom att klicka på dem. Sätt ett nytt high score!
        </p>
      </div>

      {/* Ground */}
      <Ground />
    </div>
  );
}
