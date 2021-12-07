import { loadEnvConfig } from '@next/env'
import { resolve } from 'path'

/**
 * Setup tasks called once before all Jest projects run
 */
export default async () => {

    /**
     * Load test env variables
     */
    const envFile = resolve(__dirname, '.');

    loadEnvConfig(envFile);

    /**
     * Start a server for automated tests
     */
    global.testServer = require('child_process').exec('node ./server.js');

    console.log(`Test server started`);

}
