import { useRouter } from 'next/router';
import ErrorPage from '@pages/500.page';
import { withApplicationInsights } from 'next-applicationinsights';

import { getEnvVar } from '@helpers/util/env';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { AdminLayout } from '@components/_pageLayouts/AdminLayout';
import { layoutIds } from '@constants/routes';

import '../UI/scss/screen.scss';

const App = ({ Component, pageProps }) => {

    const router = useRouter();
    const { errors, layoutId } = pageProps;

    if(errors?.find((error) => error[500])){

        return <ErrorPage statusCode={500} />

    }

    if(layoutId === layoutIds.GROUP){

        return <GroupLayout {...pageProps}><Component {...pageProps} key={router.asPath} /></GroupLayout>

    }

    if(layoutId === layoutIds.ADMIN){

        return <AdminLayout {...pageProps}><Component {...pageProps} key={router.asPath} /></AdminLayout>

    }

    return <StandardLayout {...pageProps}><Component {...pageProps} key={router.asPath} /></StandardLayout>
    
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
