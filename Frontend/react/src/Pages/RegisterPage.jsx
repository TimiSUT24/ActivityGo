import RegisterForm from "../Components/RegisterForm";
import '../CSS/RegisterForm.css';

export default function RegisterPage(){


    return (
        <div className ="register-page">
            <div className ="register-wrapper">
                <div className = "register-header">
                <h2 className = "register-title">Create an Account</h2>
                <p className ="register-subtitle">Join the adventure</p>
            </div>
            </div>         
            <RegisterForm></RegisterForm>
        </div>
    )
}