// src/hooks/useMousePosition.js
import { useEffect, useRef } from "react";

export default function useMousePosition() {
  const mouseRef = useRef({ x: window.innerWidth / 2, y: window.innerHeight / 2 });
  useEffect(() => {
    const onMove = (e) => (mouseRef.current = { x: e.clientX, y: e.clientY });
    window.addEventListener("mousemove", onMove);
    return () => window.removeEventListener("mousemove", onMove);
  }, []);
  return mouseRef;
}