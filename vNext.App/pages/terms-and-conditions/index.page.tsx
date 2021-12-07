import { GetServerSideProps } from 'next';

import { GenericContentPage } from '@components/GenericContentPage';

import { Props } from './interfaces';

const Index: (props: Props) => JSX.Element = ({
    user,
    content
}) => {

    const isAuthenticated: boolean = Boolean(user);

    return (

        <GenericContentPage 
            isAuthenticated={isAuthenticated}
            content={content}/>

    )

}

export const getServerSideProps: GetServerSideProps = async ({}) => {

    return {
        props: {
            content: {
                metaDescriptionText: 'The terms and conditions for using Future NHS',
                titleText: 'Terms and conditions',
                mainHeadingHtml: 'Terms and conditions'
            }
        }
    }

}

export default Index;