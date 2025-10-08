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
                <NavLink to="/" className ="nav-link" id="nav1">
                {location.pathname === '/' && ( //Show image only in homePage
                <img src="/IMG/Mario-Mushroom-Step-10.webp" alt="" height={20} width={20}/>
                )}  Hem</NavLink>  

                <NavLink to="/" className ="nav-link" id="nav2">Browse Activities</NavLink>
                <NavLink to="/" className ="nav-link" id="nav3">My Bookings</NavLink>
                <NavLink to="/login" className ="nav-link" id="nav4">Logga in</NavLink>
                <NavLink to="/register" className ="nav-link" id="nav5">Register</NavLink>
                </div>   
            </div>
               
        </nav>
    )
}