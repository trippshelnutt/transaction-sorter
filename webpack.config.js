module.exports = (env) => {
  if (env.production) {
    console.log(env);
    return require("./webpack.config.prod");
  }
  if (env.development) {
    console.log(env);
    return require("./webpack.config.dev");
  }
};
