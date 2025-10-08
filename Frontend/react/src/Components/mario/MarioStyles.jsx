export default function MarioStyles() {
    return (
      <style>{`
        @keyframes floatCloud { 0% { transform: translateX(-20vw); } 100% { transform: translateX(120vw); } }
        @keyframes coinSpin { 0% { transform: rotateY(0) translateY(0); } 50% { transform: rotateY(180deg) translateY(-8px); } 100% { transform: rotateY(360deg) translateY(0); } }
        @keyframes blockBounce { 0%,100% { transform: translateY(0); } 30% { transform: translateY(-6px); } 60% { transform: translateY(-2px); } }
        @keyframes groundWave { 0% { background-position: 0 0; } 100% { background-position: 80px 0; } }
  
        .cloud { position:absolute; top:8vh; width:120px; height:60px; background:#fff; border-radius:60px; filter: drop-shadow(0 3px 0 rgba(0,0,0,0.2)); opacity:0.9; animation: floatCloud linear infinite; }
        .cloud:before, .cloud:after { content:""; position:absolute; background:#fff; width:70px; height:70px; border-radius:50%; top:-20px; }
        .cloud:before { left:10px; } .cloud:after { left:50px; }
  
        .coin { position:absolute; width:28px; height:28px; border-radius:50%; background: radial-gradient(circle at 35% 35%, #fff38a 12%, #ffd54f 35%, #ffc107 60%, #ffb300 100%); border:2px solid #a66b00; box-shadow: 0 2px 0 #a66b00, 0 4px 0 #000; animation: coinSpin 1.4s linear infinite; transform-origin:center; cursor:pointer; transition: transform 180ms ease, opacity 180ms ease; }
        .coin.collected { transform: scale(0.4) translateY(-10px); opacity:0.2; pointer-events:none; }
  
        .block { position:absolute; width:42px; height:42px; background:
          radial-gradient(circle at 50% 40%, #ffd54f 18%, transparent 19%),
          linear-gradient(90deg, #8d6e63 0 3px, transparent 3px),
          linear-gradient(180deg, #8d6e63 0 3px, transparent 3px),
          linear-gradient(90deg, #5d4037, #8d6e63);
          border:3px solid #3e2723; box-shadow:0 3px 0 #000; border-radius:6px; animation: blockBounce 2.6s ease-in-out infinite; }
  
        .ground { position:absolute; bottom:0; left:0; right:0; height:70px; background: repeating-linear-gradient(45deg, #795548, #795548 10px, #5d4037 10px, #5d4037 20px); border-top:4px solid #3e2723; animation: groundWave 10s linear infinite; }
  
        .panel { position:fixed; top:70px; left:10px; right:10px; display:flex; gap:10px; justify-content:space-between; align-items:center; background:rgba(0,0,0,0.3); border:3px solid #000; border-radius:10px; padding:10px 12px; box-shadow:4px 4px 0 #000; z-index:5; }
        .score { font-size:12px; color:#ffeb3b; text-shadow:2px 2px #000; }
  
        .logout-btn { font-family:'Press Start 2P', cursive; font-size:12px; color:#fff; background: linear-gradient(180deg, #ff5252 0%, #d50000 100%); border:3px solid #000; border-radius:12px; padding:14px 28px; box-shadow:4px 4px 0 #000; cursor:pointer; transition: transform 120ms ease, background 120ms ease, box-shadow 120ms ease; }
        .logout-btn:hover { transform: translateY(-3px); background: linear-gradient(180deg, #ff867f 0%, #c62828 100%); box-shadow:6px 6px 0 #000; }
        .logout-btn:active { transform: translateY(2px); box-shadow:2px 2px 0 #000; }
  
      `}</style>
    );
  }
