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
                titleText: 'Privacy policy',
                metaDescriptionText: 'Read our privacy policy',
                mainHeadingHtml: 'Privacy policy'
            }
        }
    }

}

export default Index;