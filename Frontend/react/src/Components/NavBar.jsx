import {NavLink, useLocation} from 'react-router-dom';
import '../CSS/Navbar.css';

export default function NavBar(){
    const location = useLocation();

    const pageClass = 
        location.pathname === "/" ? "home-navbar" :
        location.pathname === "/login" ? "login-navbar" : 
        location.pathname === "/register" ? "register-navbar" : "default-navbar";
    return (
        <div>
        <nav className ={`nav-bar ${pageClass}`}style={{ display: "flex", gap: 12, padding: 12 }}>
        <NavLink to="/" className ="nav-link"><img src="./IMG/icons8-coin-100.png" alt="" height={20} />Hem</NavLink>
        <NavLink to="/login" className ="nav-link">Logga in</NavLink>
        <NavLink to="/register" className ="nav-link">Register</NavLink>
        </nav>
        </div>
    )
}