import { createContext, useContext, useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import userManager from '../AuthFiles/authConfig';
import config from '../config.json';

const AuthContext = createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isAuthorized, setIsAuthorized] = useState(false);
   
    const setLoadingState = (isLoading) => setLoading(isLoading);
    const setIsAuthorizedState = (isAuth) => setIsAuthorized(isAuth);
    const setUserState = (newUser) => setUser(newUser);
    const setUserDataState = (newUserData) => setUserData(newUserData);

    async function fetchUserData(accessToken) {
        try {
            const response = await fetch(`${config.apiBaseUrl}/GetUserProfile`, {
                headers: { 'Authorization': `Bearer ${accessToken}` }
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            return await response.json();
        } catch (error) {
            console.log('Error while sending the request to the UserService');
            return null;
        }
    }

   
    useEffect(() => {
         userManager.events.addAccessTokenExpired(() => {
            console.log('Token expired');
            userManager.signinSilent().catch(error => {
                console.log('Silent sign-in after token expiration failed', error);
            });
         });

        const checkAuth = async () => {
            setLoading(true);
            const user = await userManager.getUser();
            if (user) {
                setUser(user);
                const userData = await fetchUserData(user.access_token);
                if (userData) {
                    setIsAuthorized(true);
                    setUserData(userData);
                } else {
                    setIsAuthorized(false);
                }
            } else {
                setIsAuthorized(false);
            }
        };
        checkAuth();
        setLoading(false);
    }, []);
    

    const value = {
        user,
        userData,
        loading,
        isAuthorized,
        setLoadingState,
        setUserState,
        setUserDataState
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

AuthProvider.propTypes = {
    children: PropTypes.node.isRequired
};
