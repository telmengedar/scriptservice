declare let ENV_VARS: {[key: string]: string};

export const environment = {
  requiresLogin: true,
  production: true,
  apiUrl: 'https://dev.mamgo.io/api',
  environment: ENV_VARS
};
