export function spawnCoin() {
    const vw = Math.max(360, window.innerWidth);
    const vh = Math.max(520, window.innerHeight);
    const margin = 80;
    return {
      id: crypto.randomUUID(),
      x: Math.random() * (vw - margin * 2) + margin,
      y: Math.random() * (vh - 220) + 120,
      collected: false,
    };
  }