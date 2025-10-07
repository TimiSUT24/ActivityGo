import {useState} from 'react';
import axios from 'axios';
import '../CSS/RegisterForm.css';

export default function RegisterForm (){

    const [formData, setFormData] = useState({
        email:"",
        password:"",
        firstname: "",
        lastname: ""
    });

    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        setFormData({...formData, [e.target.name]: e.target.value}) // updates state fromData when typing email for example 
    }

    const handleSubmit = async (e) => {
        e.preventDefault();

        if(!formData.email || !formData.password){
            setMessage("Email and password required")
            return;
        }

    try{

        await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Auth/register`,formData);
        
        setMessage("Registration successful!")
        }
        catch(error)
        {
            setMessage(error.response?.data || "Registration failed")
        }
    }

    return (
        <div>
            <form onSubmit = {handleSubmit} className = "register-form">
                <input 
                type = "text" 
                name = "email"
                placeholder = "email"
                value = {formData.email}
                onChange = {handleChange}
                className = "register-input"
                />

                <input 
                type="password" 
                name = "password"
                placeholder = "password"
                value = {formData.password}
                onChange = {handleChange}
                className = "register-input"
                />

                <input 
                type="text" 
                name = "firstname"
                placeholder = "firstname"
                value = {formData.firstname}
                onChange = {handleChange}
                className = "register-input"
                />

                <input 
                type="text" 
                name = "lastname"
                placeholder = "lastname"
                value = {formData.lastname}
                onChange = {handleChange}
                className = "register-input"
                />

                <button type = "submit" className="register-button">Register</button>

                {message && <p>{message}</p>}
            </form>
        </div>
    )
}