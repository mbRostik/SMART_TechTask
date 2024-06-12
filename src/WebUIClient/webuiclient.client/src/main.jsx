import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.jsx'
import './index.css'

import { AuthProvider } from './Components/AuthProvider';
import { useNavigate } from 'react-router-dom';
import SignIn_CallbackPage from './AuthFiles/SignIn_CallbackPage';
import SignOut_CallBackPage from './AuthFiles/SignOut_CallBackPage';
import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, useLocation } from 'react-router-dom';


const root = ReactDOM.createRoot(document.getElementById('root'));

function AppContainer() {
    return (
        <div className="All_container">
            <Routes>
                <Route path="/signin-oidc" element={<SignIn_CallbackPage />} />
                <Route path="/signout-callback-oidc" element={<SignOut_CallBackPage />} />
                <Route path="/" element={<App />} />
            </Routes>
        </div>
    );
}

root.render(
    <React.StrictMode>
        <AuthProvider>
                <Router>
                    <AppContainer />
                </Router>
        </AuthProvider>
    </React.StrictMode>
);