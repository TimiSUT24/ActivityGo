import {NavLink, useLocation} from 'react-router-dom';
import {useEffect} from 'react';
import '../CSS/Navbar.css';

export default function NavBar(){
    const location = useLocation();

    useEffect(() => {
    const bodyClassMap = {
        "/": "home-body",
        "/login": "login-body",
        "/register": "register-body",
    };

    const newClass = bodyClassMap[location.pathname] || "default-body";

    document.body.classList.remove("home-body", "login-body", "register-body", "default-body");

    document.body.classList.add(newClass);

    return () => document.body.classList.remove(newClass);
    }, [location]);

    const pageClass = 
        location.pathname === "/" ? "home-navbar" :
        location.pathname === "/login" ? "login-navbar" : 
        location.pathname === "/register" ? "register-navbar" : "default-navbar";
    return (
        <nav className ={`nav-bar ${pageClass}`}style={{padding: 12 }}>
            <div className="navbar-container">
                <div className="nav-img">   
                    <img src="/IMG/Activigotitle.png" alt="" />                 
                </div>
                <div className="nav-links">                 
                <NavLink to="/" className ="nav-link" id="nav1">
                {location.pathname === '/' && ( //Show image only in homePage
                <img src="/IMG/Mario-Mushroom-Step-10.webp" alt="" height={20} width={20}/>
                )}  Hem</NavLink>  

                <NavLink to="/" className ="nav-link" id="nav2">
                {location.pathname === '/' && ( 
                <img src="/IMG/icons8-pixel-star-48.png" alt="" height={20} width={20}/>
                )}Sök Aktivitet</NavLink>

                <NavLink to="/" className ="nav-link" id="nav3">
                {location.pathname === '/' && ( 
                <img src="/IMG/bookinicon.png" alt="" height={20} width={20}/>
                )}Mina Bokningar</NavLink>

                <NavLink to="/login" className ="nav-link" id="nav4">Logga in</NavLink>
                <NavLink to="/register" className ="nav-link" id="nav5">Registrera</NavLink>
                </div>   
            </div>              
        </nav>
    )
}