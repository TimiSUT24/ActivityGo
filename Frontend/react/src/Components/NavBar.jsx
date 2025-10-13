import { NavLink, useLocation } from "react-router-dom";
import { useEffect } from "react";
import "../CSS/Navbar.css";

export default function NavBar() {
  const location = useLocation();

  useEffect(() => {
    const bodyClassMap = {
      "/": "home-body",
      "/login": "login-body",
      "/register": "register-body",
      "/activity-occurrences": "activity-occurrences-body",
    };

    const newClass = bodyClassMap[location.pathname] || "default-body";

    document.body.classList.remove(
      "home-body",
      "login-body",
      "activity-occurrences-body",
      "register-body",
      "default-body"
    );

    document.body.classList.add(newClass);

    return () => document.body.classList.remove(newClass);
  }, [location]);

  const pageClass =
    location.pathname === "/"
      ? "home-navbar"
      : location.pathname === "/activity-occurrences"
      ? "activity-occurrences-navbar"
      : location.pathname === "/login"
      ? "login-navbar"
      : location.pathname === "/user"
      ? "user-navbar"
      : location.pathname === "/register"
      ? "register-navbar"
      : "default-navbar";
  return (
    <nav className={`nav-bar ${pageClass}`} style={{ padding: 12 }}>
      <div className="navbar-container">
        <div className="nav-img">
          <img src="/IMG/Activigotitle.png" alt="" className="activigo-title" />
        </div>
        <div className="nav-links">
          <NavLink to="/" className="nav-link" id="nav1">
            {(location.pathname === "/" || location.pathname === "/user") && ( //Show image only in homePage/user
              <img
                src="/IMG/Mario-Mushroom-Step-10.webp"
                alt=""
                height={20}
                width={20}
              />
            )}{" "}
            Hem
          </NavLink>

          <NavLink to="/activity-occurrences" className="nav-link" id="nav2">
            {(location.pathname === "/" || location.pathname === "/user") && (
              <img
                src="/IMG/icons8-pixel-star-48.png"
                alt=""
                height={20}
                width={20}
              />
            )}
            Sök Aktivitet
          </NavLink>

          {location.pathname === "/user" && (
            <>
              <NavLink to="/user" className="nav-link" id="nav3">
                <img src="/IMG/bookinicon.png" alt="" height={20} width={20} />
                Mina Bokningar
              </NavLink>
            </>
          )}

          <NavLink to="/user" className="nav-link" id="nav6">
            Mina Sidor
          </NavLink>

          {location.pathname !== "/user" && (
            <>
              <NavLink to="/login" className="nav-link" id="nav4">
                Logga in
              </NavLink>
              <NavLink to="/register" className="nav-link" id="nav5">
                Registrera
              </NavLink>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
