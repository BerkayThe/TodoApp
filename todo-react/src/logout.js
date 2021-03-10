import { useContext, useEffect } from 'react';
import AppContext from './AppContext';
import { Redirect } from 'react-router-dom';

function Logout() {
    const ctx = useContext(AppContext);

    useEffect(() => {
        localStorage.removeItem("token");
        ctx.setToken(null);
        ctx.setLoggedIn(false);
    });


    return (
        <Redirect to="/login" />
    );

}

export default Logout;