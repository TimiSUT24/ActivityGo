import { Link } from "react-router-dom";
import LocalWeather from "../Components/LocalWeather";
import "../CSS/HomePage.css";
import "../CSS/Occurrences.css";

export default function HomePage() {
  return (
    <div className="home-page">
      <section className="home-hero brick-frame">
        <div className="home-hero__copy">
          <p className="home-eyebrow">Välkommen till ActivityGo</p>
          <h1 className="home-title mario-title">
            Planera ditt nästa äventyr
          </h1>
          <p className="home-lead">
            Upptäck aktiviteter, se lediga tillfällen i realtid och boka på
            sekunder. Allt i en färgsprakande Mario-värld.
          </p>

          <div className="home-hero__actions">
            <Link to="/activities" className="home-btn home-btn--primary">
              Utforska aktiviteter
            </Link>
            <Link to="/activity-occurrences" className="home-btn home-btn--ghost">
              Hitta tillfällen
            </Link>
          </div>

          <ul className="home-hero__perks">
            <li>🎯 Filtrera aktiviteter efter kategori, plats och miljö</li>
            <li>⚡ Boka på sekunder och följ dina aktiviteter i realtid</li>
            <li>🌦 Live-väder för utomhusäventyr</li>
          </ul>
        </div>

        <img
          src="/IMG/HomePage/marioisland.png"
          alt="Marioö full av aktiviteter"
          className="home-hero__art"
          width={420}
          height={320}
        />
      </section>

      <section className="home-highlights">
        <article className="home-highlight brick-frame">
          <img
            src="/IMG/HomePage/Greencircle.png"
            alt=""
            className="home-highlight__icon"
            width={80}
            height={80}
          />
          <h2 className="home-highlight__title">Aktiviteter för alla</h2>
          <p>
            Sport, familjeäventyr eller relax – vi har något för varje humör och
            ålder.
          </p>
        </article>

        <article className="home-highlight brick-frame">
          <img
            src="/IMG/HomePage/mushroomcircle.png"
            alt=""
            className="home-highlight__icon"
            width={80}
            height={80}
          />
          <h2 className="home-highlight__title">Enkel bokning</h2>
          <p>
            Se lediga platser och boka på ett ögonblick. Vi håller koll på
            dina bokningar åt dig.
          </p>
        </article>

        <article className="home-highlight brick-frame">
          <img
            src="/IMG/HomePage/bluecircle.png"
            alt=""
            className="home-highlight__icon"
            width={80}
            height={80}
          />
          <h2 className="home-highlight__title">Logga in på dina sidor</h2>
          <p>
            Logga in för att se alla dina kommande bokningar och få full
            historik över det du redan hunnit göra.
          </p>
        </article>
      </section>

      <section className="home-how brick-frame">
        <h2 className="home-section-title mario-title">Så funkar det</h2>
        <ol className="home-steps">
          <li className="home-step">
            <span className="home-step__number">1</span>
            <div>
              <h3>Skapa konto eller logga in</h3>
              <p>Det går snabbt, och du får din egen aktivitetsöversikt.</p>
            </div>
          </li>
          <li className="home-step">
            <span className="home-step__number">2</span>
            <div>
              <h3>Filtrera & hitta rätt</h3>
              <p>
                Välj kategori, datum, plats eller miljö för att hitta perfekta
                tillfällen.
              </p>
            </div>
          </li>
          <li className="home-step">
            <span className="home-step__number">3</span>
            <div>
              <h3>Boka och följ upp</h3>
              <p>Boka din plats och se dina kommande aktiviteter på ett ställe.</p>
            </div>
          </li>
        </ol>
      </section>

      <section className="home-cta brick-frame">
        <div>
          <h2 className="home-section-title">Redo att hoppa in?</h2>
          <p>
            Starta med att utforska våra aktiviteter eller gå direkt till
            kalendern för att boka ett tillfälle som passar dig.
          </p>
        </div>
        <div className="home-cta__actions">
          <Link to="/activities" className="home-btn home-btn--primary">
            Visa aktiviteter
          </Link>
          <Link to="/activity-occurrences" className="home-btn home-btn--accent">
            Kalender med tillfällen
          </Link>
        </div>
      </section>
    </div>
  );
}
