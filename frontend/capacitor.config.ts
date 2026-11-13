import type {CapacitorConfig} from '@capacitor/cli';

const config: CapacitorConfig = {
    appId: 'za.co.levelupnow.app',
    appName: 'LevelUp',
    webDir: 'out',
    backgroundColor: '#000000',
    server: {
        androidScheme: 'https',
        // Match the production frontend origin already permitted by the API.
        hostname: 'levelupnow.co.za',
    },
};

export default config;
