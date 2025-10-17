import { NavLink, useLocation } from 'react-router-dom';
import { useEffect, useState} from 'react';
import '../CSS/Navbar.css';
import LocalWeather from '../Components/LocalWeather';
import { useAuth } from '../context/AuthContext'; // du har redan denna i ditt projekt

export default function NavBar() {
  const location = useLocation();
  const { user } = useAuth(); // t.ex. { email, role } eller { roles: [] }
  const [menuOpen, setMenuOpen] = useState(false);

  const toggleMenu = () => {
    setMenuOpen(prev => !prev)
  }

  useEffect(() => {
    setMenuOpen(false)
  }, [location])

  // ✅ Kolla roll
  const isAdmin =
    !!user &&
    (
      (Array.isArray(user.roles) && user.roles.includes('Admin')) ||
      user.role === 'Admin'
    );

  const isUser =
    !!user &&
    (
      (Array.isArray(user.roles) && user.roles.includes('User')) ||
      user.role === 'User'
    );

  // Body class logik (samma som innan)
  useEffect(() => {
    const bodyClassMap = {
      "/": "home-body",
      "/login": "login-body",
      "/register": "register-body",
      "/activity-occurrences": "activity-occurrences-body",
      "/activities": "activity-occurrences-body",
      "/admin": "admin-body",
      "/me/bookings": "booking-body",
      "/user": "user-body"
    };
    const newClass = bodyClassMap[location.pathname] || "default-body";
    document.body.classList.remove("home-body", "login-body", "register-body", "admin-body", "activity-occurrences-body", "booking-body", "user-body", "default-body");
    document.body.classList.add(newClass);
    return () => document.body.classList.remove(newClass);
  }, [location]);

  const pageClass =
    location.pathname === "/" ? "home-navbar" :
    location.pathname === "/login" ? "login-navbar" :
    location.pathname === "/user" ? "user-navbar" :
    location.pathname === "/register" ? "register-navbar" :
    location.pathname === "/activity-occurrences" ? "activity-occurrences-navbar" :
    location.pathname === "/activities" ? "activity-occurrences-navbar" :
    location.pathname === "/admin" ? "admin-navbar" :
    "default-navbar";

  // ✅ Funktion som bestämmer vilka länkar som ska visas
  const renderLinks = () => {
    // --- ADMIN ---
    if (isAdmin) {
      return (
        <>
          <NavLink to="/" className="nav-link" id="nav1"><img src="/IMG/Mario-Mushroom-Step-10.webp" alt="" width={20} height={20}/>Hem</NavLink>
          <NavLink to="/activities" className="nav-link" id="nav-activities"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Aktiviteter</NavLink>
          <NavLink to="/activity-occurrences" className="nav-link" id="nav2"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Sök Aktivitet</NavLink>
          <NavLink to="/user" className="nav-link" id="nav6">Mina Sidor</NavLink>
          <NavLink to="/admin" className="nav-link" id="nav-admin">Admin</NavLink>
        </>
      );
    }

    // --- INLOGGAD USER ---
    if (isUser) {
      return (
        <>
          <NavLink to="/" className="nav-link" id="nav1"><img src="/IMG/Mario-Mushroom-Step-10.webp" alt="" width={20} height={20}/>Hem</NavLink>
          <NavLink to="/activities" className="nav-link" id="nav-activities"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Aktiviteter</NavLink>
          <NavLink to="/activity-occurrences" className="nav-link" id="nav2"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Sök Aktivitet</NavLink>
          <NavLink to="/me/bookings" className="nav-link" id="nav3"><img src="/IMG/bookinicon.png" alt="" width={20} height={20}/>Mina Bokningar</NavLink>
          <NavLink to="/user" className="nav-link" id="nav6">Mina Sidor</NavLink>
        </>
      );
    }

    // --- UTSKRIVEN / EJ INLOGGAD ---
    return (
      <>
        <NavLink to="/" className="nav-link" id="nav1"><img src="/IMG/Mario-Mushroom-Step-10.webp" alt="" width={20} height={20}/>Hem</NavLink>
        <NavLink to="/activities" className="nav-link" id="nav-activities"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Aktiviteter</NavLink>
        <NavLink to="/activity-occurrences" className="nav-link" id="nav2"><img src="/IMG/icons8-pixel-star-48.png" alt="" width={20} height={20}/>Sök Aktivitet</NavLink>
        <NavLink to="/login" className="nav-link" id="nav4">Logga in</NavLink>
        <NavLink to="/register" className="nav-link" id="nav5">Registrera</NavLink>
      </>
    );
  };

  return (
    <nav className={`nav-bar ${pageClass}`} style={{ padding: 12 }}>
      <div className="navbar-container">
        <div className="nav-img">
          <img src="/IMG/Activigotitle.png" alt="Activigo" className="activigo-title" />
        </div>
        <button className ="menu-toggle" id="menu-toggle" onClick={toggleMenu}>☰</button>
        <div className={`nav-links ${menuOpen ? 'open' : ''}`}>
          {renderLinks()}
        </div>
        <div className="nav-weather">
          <LocalWeather />
        </div>
      </div>
    </nav>
  );
}
