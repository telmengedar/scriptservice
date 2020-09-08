const webpack = require('webpack');

module.exports = {
    plugins: [new webpack.DefinePlugin({
        'ENV_VARS': {
            AUTH_URL: JSON.stringify(process.env.AUTH_URL),
            CLIENT_ID: JSON.stringify(process.env.CLIENT_ID),
            CLIENT_SECRET: JSON.stringify(process.env.CLIENT_SECRET)
        }
    })]
}