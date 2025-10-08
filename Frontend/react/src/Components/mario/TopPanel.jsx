export default function TopPanel({ score, username, onLogout }) {
  return (
    <div className="panel">
      <div className="score">SCORE: {String(score).padStart(3, "0")}</div>
      <div style={{ fontSize: 10 }}>👋 {username}</div>
      <button className="logout-btn" onClick={onLogout}>🪙 Logga ut</button>
    </div>
  );
}
