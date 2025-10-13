export default function QuestionBlock({ left, top, delay = "0s" }) {
    return <div className="block" style={{ left, top, animationDelay: delay }} />;
  }