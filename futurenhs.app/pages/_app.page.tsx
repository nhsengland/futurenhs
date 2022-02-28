import { useRouter } from 'next/router';
import ErrorPage from '@pages/500.page';
import { withApplicationInsights } from 'next-applicationinsights';

import { getEnvVar } from '@helpers/util/env';
import '../UI/scss/screen.scss';

const App = ({ Component, pageProps }) => {

    const router = useRouter();
    const { errors } = pageProps;

    if(errors?.find((error) => error[500])){

        return <ErrorPage statusCode={500} />

    }

    return <Component {...pageProps} key={router.asPath} />
    
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