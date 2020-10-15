declare let ENV_VARS: {[key: string]: string};

export const environment = {
  requiresLogin: true,
  production: true,
  environment: ENV_VARS
};
