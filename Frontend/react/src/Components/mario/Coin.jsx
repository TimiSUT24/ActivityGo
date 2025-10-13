export default function Coin({ x, y, collected, onClick }) {
    return (
      <div
        className={`coin ${collected ? "collected" : ""}`}
        style={{ left: x, top: y }}
        onClick={onClick}
        title="Samla mynt!"
      />
    );
  }