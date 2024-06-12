import { UserManager  } from 'oidc-client';

const config = {
    authority: 'https://localhost:7174',
    client_id: 'interactive',
    client_secret: 'OnlyUserKnowsThisSecret',
    redirect_uri: 'https://localhost:5173/signin-oidc',
    response_type: 'code',
    scope: 'openid profile OutOfOffice.WebApi.Scope',
    post_logout_redirect_uri: 'https://localhost:5173/signout-callback-oidc',
};

const userManager = new UserManager(config);

export default userManager;