var path = require("path");

module.exports = {
  entry: {
    index: "./Scripts/index.ts",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
  },
  resolve: {
    alias: {
      vue: "vue/dist/vue.min.js",
    },
  },
  output: {
    filename: "bundle.js",
    publicPath: "/js/",
    path: path.join(__dirname, "/wwwroot/js/"),
  },
};
