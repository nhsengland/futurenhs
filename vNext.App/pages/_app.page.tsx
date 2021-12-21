import { withApplicationInsights } from 'next-applicationinsights';
import { getEnvVar } from '@helpers/util/env';
import '../UI/scss/screen.scss';

const App = ({ Component, pageProps }) => {

    return <Component {...pageProps} key={pageProps.id} />
    
}

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
