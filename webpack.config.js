module.exports = (env, argv) => {
    if (argv.mode === 'production') {
        return require('./webpack.config.prod');
    }
    if (argv.mode === 'development') {
        return require('./webpack.config.dev');
    }
}
