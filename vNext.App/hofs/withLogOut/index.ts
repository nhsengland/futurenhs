import { GetServerSideProps, GetServerSidePropsContext } from 'next';

export const withLogOut: (getServerSideProps: GetServerSideProps) => GetServerSideProps = (getServerSideProps: GetServerSideProps) => {

    return async (context: GetServerSidePropsContext): Promise<any> => {

        for (var prop in context.req.cookies) {

            (context.res as any).cookie(prop, '', {
                ['expires']: new Date(0),
                ['max-age']: 0
            });
    
        }

        return await getServerSideProps(context);

    }

}