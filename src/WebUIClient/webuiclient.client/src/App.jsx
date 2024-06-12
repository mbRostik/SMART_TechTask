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
        <div className="">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized === false ? (
                <div className="">
                    <div><button onClick={onLogin} className="">Login</button></div>
                    <div><button onClick={onLogin} className="">Sign Up</button></div>
                </div>
            ) : userData === null ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
                    ) : (
                    <div className="">
                            
                        {userData && userData.fullyRegistered ? (
                            <>
                                        
                                <div className="">
                                    <button onClick={onLogout} className="">LogOut</button>
                                </div>
                            </>
                        ) : (
                            <div className="">
                                <button onClick={onLogout} className="">LogOut</button>
                                <FinishUserRegistrationForm />
                            </div>
                        )}
                    </div>
            )}
        </div>

    );
}

export default App;