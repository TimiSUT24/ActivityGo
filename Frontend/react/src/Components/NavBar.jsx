import {NavLink, useLocation} from 'react-router-dom';
import '../CSS/Navbar.css';

export default function NavBar(){
    const location = useLocation();

    const pageClass = 
        location.pathname === "/" ? "home-navbar" :
        location.pathname === "/login" ? "login-navbar" : 
        location.pathname === "/register" ? "register-navbar" : "default-navbar";
    return (
        <nav className ={`nav-bar ${pageClass}`}style={{padding: 12 }}>
            <div className="navbar-container">
                <div className="nav-img">
                    <img src="./IMG/icons8-coin-100.png" alt="" />
                </div>
                <div className="nav-links">
                <NavLink to="/" className ="nav-link" id="nav1">Hem</NavLink>
                <NavLink to="/login" className ="nav-link" id="nav2">Logga in</NavLink>
                <NavLink to="/register" className ="nav-link" id="nav3">Register</NavLink>
                </div>   
            </div>
               
        </nav>
    )
}