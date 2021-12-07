/**
 * Teardown tasks called once after all Jest projects run
 */
export default async () => {

    /**
     * Destroy test server
     */
    console.log(`Test server destroyed`);

    global.testServer.kill('SIGINT');

    /**
     * Make sure jest exits
     */
    setTimeout(() => process.exit(), 1000);

}
