import { GetServerSideProps } from 'next';
import { GetServerSidePropsContext as NextGetServerSidePropsContext, GetServerSideProps } from 'next';

import { User } from './user';

export interface GetServerSidePropsContext extends NextGetServerSidePropsContext {
    req: NextGetServerSidePropsContext.req & {
        user?: User;
        body?: any;
    };
    page: {
        routeId: string;
        props: Record<any, any>;
    }
}