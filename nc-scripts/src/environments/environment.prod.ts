declare let ENV_VARS: {[key: string]: string};

export const environment = {
  requiresLogin: false,
  production: true,
  environment: ENV_VARS
};
