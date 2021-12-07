import { withApplicationInsights } from 'next-applicationinsights';
import '../UI/scss/screen.scss';
import { getEnvVar } from '@helpers/util/env';

const App = ({ Component, pageProps }) => <Component {...pageProps} />

export default withApplicationInsights({ 
    instrumentationKey: getEnvVar({ 
        name: 'APPINSIGHTS_INSTRUMENTATIONKEY',
        isRequired: false 
    }) as string,
    isEnabled: getEnvVar({ 
        name: 'APPINSIGHTS_INSTRUMENTATIONKEY',
        isRequired: false 
    }) && process.env.NODE_ENV === 'production'
})(App as any);
