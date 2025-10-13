export default function Cloud({ left = "-15vw", top = "8vh", width = 120, height = 60, duration = "28s" }) {
    return (
      <div
        className="cloud"
        style={{ left, top, width, height, animationDuration: duration }}
      />
    );
  }