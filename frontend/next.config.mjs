/** Configures browser implementations for the existing shared UI components. */
import path from 'node:path';
const root=process.cwd();
const config={
 output:'standalone',reactStrictMode:true,devIndicators:false,
 outputFileTracingRoot:root,
 transpilePackages:['react-native-web','react-native-svg','react-native-safe-area-context'],
 typescript:{tsconfigPath:'tsconfig.json'},
 /** Sends www visitors to the canonical host, which is the only origin the API allows. */
 async redirects(){return [{source:'/:path*',has:[{type:'host',value:'www.levelupnow.co.za'}],destination:'https://levelupnow.co.za/:path*',permanent:true}];},
 /** Resolves shared presentation components to their browser implementations. */
 webpack(config){config.resolve.alias={...config.resolve.alias,'react-native$':'react-native-web','react-native-svg$':path.join(root,'node_modules/react-native-svg/lib/module/ReactNativeSVG.web.js')};config.resolve.extensions=['.web.tsx','.web.ts','.web.js',...config.resolve.extensions];return config;}
};
export default config;
