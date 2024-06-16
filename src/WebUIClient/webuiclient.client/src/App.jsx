import { useEffect, useState } from 'react';
import './App.css';
import { Link, useNavigate } from 'react-router-dom';
import userManager from './AuthFiles/authConfig';
import { useAuth } from './Components/AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import FinishUserRegistrationForm from './Components/FinishUserRegistration/FinishUserRegistrationForm.jsx'

function App() {
    const navigate = useNavigate();
    const { user, userData, loading, isAuthorized } = useAuth();


    const onLogin = () => {
        userManager.signinRedirect();
    };

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };


    return (
        <div className="AppContainer">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized === false ? (
                    <div className="button-container">
                        <div className="button-container-buttons">
                            <div><button onClick={onLogin} className="custom-button login-button">Login</button></div>
                            <div><button onClick={onLogin} className="custom-button login-button">Sign Up</button></div>
                        </div>
                        
                        <img src="/Saitama-PNG-Image.png" alt="Avatar" className="avatar" />

                    </div>

            ) : userData === null ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
                    ) : (
                    <div className="AppMain">
                            
                        {userData && userData.fullyRegistered ? (
                            <>
                                        
                                        <div className="header-container">
                                            <h1 className="header-title">SMART business - Junior CRM Developer - Test task</h1>
                                            <img src="/Saitama-1-816x1024.png" alt="Avatar" className="avatar" />

                                            <button onClick={onLogout} className="custom-button signup-button">LogOut</button>
                                        </div>

                            </>
                        ) : (
                            <div className="RegFormApp">
                                 
                                            <FinishUserRegistrationForm />
                                       <br></br>
                                            <button onClick={onLogout} className="custom-button signup-button">LogOut</button>
                            </div>
                        )}
                    </div>
            )}
        </div>

    );
}

export default App;