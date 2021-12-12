import Head from 'next/head';

import { Props } from './interfaces'

const DocumentHead: (props: Props) => JSX.Element = ({
    basePath = ''
}) => {

    return (

        <Head>
            <link
                rel="apple-touch-icon"
                sizes="180x180"
                href={`${basePath}/favicon/favicon/apple-touch-icon.png`}
            />
            <link
                rel="icon"
                type="image/png"
                sizes="32x32"
                href={`${basePath}/favicon/favicon-32x32.png`}
            />
            <link
                rel="icon"
                type="image/png"
                sizes="16x16"
                href={`${basePath}/favicon/favicon-16x16.png`}
            />
            <link rel="manifest" href={`${basePath}/favicon/manifest.json`} />
            <link
                rel="mask-icon"
                href={`${basePath}/favicon/safari-pinned-tab.svg`}
                color="#000000"
            />
            <link rel="shortcut icon" href={`${basePath}/favicon/favicon.ico`} />
            <meta name="msapplication-TileColor" content="#000000" />
            <meta name="msapplication-config" content={`${basePath}/favicon/browserconfig.xml`} />
            <meta name="theme-color" content="#000" />
            <link rel="alternate" type="application/rss+xml" href={`${basePath}/favicon/feed.xml`} />
            <meta name="description" content={``} />
        </Head>

    )

}

export default DocumentHead;